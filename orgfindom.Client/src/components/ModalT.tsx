import { useState } from "react";
import { createPortal } from "react-dom";
import '../css/Modal.css'

interface ModalTProps<T> {
    fields: string[];
    children?: React.ReactNode;
    userAge?: number | null;
    mountElement?: string;
    secretFields: string[];
    onModalSubmit: (data: T) => void;
}


function ModalT<T>({ children, fields, secretFields, mountElement, userAge, onModalSubmit }: ModalTProps<T>) {
    const [modal, setModal] = useState(false);
    const mountE = (document.getElementById(`${mountElement}`)) || document.body;
    
    const fieldList = fields?.map((field, index) => {
        if (field === "Tipo") {
            return (
                <div key={`type-${index}`} className="modal-field-group">
                    <label className="modal-label">{field}</label>
                    <select className='modal-input' name={field} required>
                        { ((userAge ?? 18) >= 18) && (
                            <option value="Receita">Receita</option>
                        )}
                        <option value="Despesa">Despesa</option>
                    </select>
                </div>
            );
        }
        
        return (
            <label className="modal-label">
                {field}
                <input className='modal-input' placeholder={field} name={field} type="text" key={`text-${index}`}/> 
            </label>
        );
    });
    
    const secretFieldList = secretFields?.map((fieldSecret, index) => {
        return (
            <label className="modal-label">
                {fieldSecret}
                <input className='modal-input' placeholder={fieldSecret} name={fieldSecret} type="password" key={`text-${index}`}/>
            </label>
        );
    });

    const toggleModal = () => {
        setModal(!modal);
    }

    const handleContentClick = (e: React.MouseEvent) =>{
        e.stopPropagation();
    }

    const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
        e.preventDefault();

        const formData = new FormData(e.currentTarget);

        const values = Object.fromEntries(formData.entries());

        onModalSubmit(values as unknown as T);

        toggleModal();
    }

    return(
        <>
            
            <button onClick={ toggleModal } className="modal-open">{children}</button>
            
    
            { modal && mountE ? (
                createPortal(
                    <div className="modal-overlay" onClick={toggleModal}>
                        <form onSubmit={handleSubmit} onClick={handleContentClick}>
                            {fieldList}
                            {secretFieldList}
                            <button className="sub-bttn" type="submit">
                                Enviar
                            </button>
                            <button type="button" className="close-bttn" onClick={ toggleModal }>Fechar</button>
                        </form>
                    </div>,
                    mountE
                )
            ): null}
        </>
    )
}
export default ModalT;